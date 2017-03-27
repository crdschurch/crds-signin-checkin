import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../../shared/services';
import { HeaderService } from '../../../header/header.service';

@Component({
  selector: 'household',
  templateUrl: 'household.component.html',
  styleUrls: ['household.component.scss']
})
export class HouseholdComponent implements OnInit {
  private eventId: string;
  private householdId: string;
  private processing: boolean;

  constructor( private apiService: ApiService,
               private route: ActivatedRoute,
               private headerService: HeaderService) {}

 ngOnInit() {
   this.processing = false;
   this.eventId = this.route.snapshot.params['eventId'];
   this.householdId = this.route.snapshot.params['householdId'];

   this.apiService.getEvent(this.eventId).subscribe((event) => {
     this.headerService.announceEvent(event);
   }, error => console.error(error));
 }

}
