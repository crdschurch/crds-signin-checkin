import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { ApiService, RootService } from '../../../../../shared/services';
import { HeaderService } from '../../../../header/header.service';
import { AdminService } from '../../../../admin.service';
import { Household, State, Country } from '../../../../../shared/models';

@Component({
  selector: 'household-edit',
  templateUrl: 'household-edit.component.html'
})
export class HouseholdEditComponent implements OnInit {
  private maskPhoneNumber: any = [/[1-9]/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/];
  private loading: boolean;
  private processing: boolean;
  private eventId: number;
  private householdId: number;
  private household: Household = new Household();
  private states: Array<State> = [];
  private countries: Array<Country> = [];

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private route: ActivatedRoute,
               private headerService: HeaderService,
               private rootService: RootService,
               private router: Router) {}

 ngOnInit() {
   this.loading = true;
   this.processing = false;
   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];

   this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
     this.headerService.announceEvent(event);
   }, error => console.error(error));

   this.adminService.getStates().subscribe((states) => {
     this.states = states;

     this.adminService.getCountries().subscribe((countries) => {
        this.countries = countries;

        this.adminService.getHouseholdInformation(this.householdId).subscribe((household) => {
         this.household = household;
         this.loading = false;
        }, error => console.error(error));
     }, error => console.error(error));
   }, error => console.error(error));
 }

  onPhoneBlur(e, household) {
    try {
      if (household.HomePhone.indexOf('_') > -1) {
        e.target.value = '';
        household.HomePhone = undefined;
      }
    } catch (e) { }
  }

  updateState(newState) {
    this.household.State = newState;
  }

  updateCountry(newCountry) {
    this.household.CountryCode = newCountry;
  }

  onSave(form: NgForm) {
    if (form.valid) {
      this.processing = true;
      this.adminService.updateHousehold(this.household).subscribe((res) => {
        this.rootService.announceEvent('echeckNewFamilyCreated');
        this.processing = false;
        this.router.navigate(['/admin/events', this.eventId, 'family-finder', this.householdId]);
      }, (error) => {
        this.rootService.announceEvent('generalError');
        this.processing = false;
      });
    }
  }

  onSubmit(form: NgForm) {
    if (form.valid) {
      this.processing = true;
      this.adminService.updateHousehold(this.household).subscribe((res) => {
        this.rootService.announceEvent('echeckNewFamilyCreated');
        this.processing = false;
      }, (error) => {
        this.rootService.announceEvent('generalError');
        this.processing = false;
      });
    }
  }
}
