import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { ApiService } from '../../../../../shared/services';
import { HeaderService } from '../../../../header/header.service';

@Component({
  selector: 'household-edit',
  templateUrl: 'household-edit.component.html'
})
export class HouseholdEditComponent implements OnInit {
  private eventId: number;
  private householdId: number;
  private processing: boolean;

  constructor( private apiService: ApiService,
               private route: ActivatedRoute,
               private headerService: HeaderService) {}

 ngOnInit() {
   this.processing = true;
   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];

   this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
     this.headerService.announceEvent(event);
   }, error => console.error(error));
 }
}
