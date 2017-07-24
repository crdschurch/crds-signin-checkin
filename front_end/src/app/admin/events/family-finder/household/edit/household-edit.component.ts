import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { NgForm } from '@angular/forms';

import { RootService } from '../../../../../shared/services';
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
  isNewFamily: boolean;

  constructor( private adminService: AdminService,
               private route: ActivatedRoute,
               private rootService: RootService,
               private router: Router,
               private location: Location) {}

 ngOnInit() {
   this.loading = true;
   this.processing = false;

   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];
   this.isNewFamily = this.route.snapshot.params['newFamily'];

   this.adminService.getStates().subscribe((states) => {
     this.states = states;

     this.adminService.getCountries().subscribe((countries) => {
        this.countries = countries;

        this.adminService.getHouseholdInformation(this.householdId).subscribe((household) => {
         this.household = household;
         // default to USA
         if (!this.household.CountryCode) {
           this.household.CountryCode = 'USA';
         }
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

  onSkip() {
    this.router.navigate(['/admin/events', this.eventId, 'family-finder', this.householdId]);
  }

  onSave(form: NgForm) {
    if (form.valid && form.dirty) {
      this.processing = true;
      this.adminService.updateHousehold(this.household).subscribe((household) => {
        this.rootService.announceEvent('echeckFamilyFinderHouseholdUpdateSuccessful');
        this.household = household;
        this.processing = false;
        this.router.navigate(['/admin/events', this.eventId, 'family-finder', this.householdId]);
      }, (error) => {
        this.rootService.announceEvent('generalError');
        this.processing = false;
      });
    } else if (form.pristine) {
      this.router.navigate(['/admin/events', this.eventId, 'family-finder', this.householdId]);
    }
  }

  onSubmit(form: NgForm) {
    if (form.valid && form.dirty) {
      this.processing = true;
      this.adminService.updateHousehold(this.household).subscribe((household) => {
        this.household = household;
        this.processing = false;
        this.rootService.announceEvent('echeckFamilyFinderHouseholdUpdateSuccessful');
      }, (error) => {
        this.rootService.announceEvent('generalError');
        this.processing = false;
      });
    }
  }

  back() {
    this.location.back();
  }
}
