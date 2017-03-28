import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../../shared/services';
import { AdminService } from '../../../admin.service';
import { Child } from '../../../../shared/models';
import { HeaderService } from '../../../header/header.service';

@Component({
  selector: 'household',
  templateUrl: 'household.component.html',
  styleUrls: ['household.component.scss']
})
export class HouseholdComponent implements OnInit {
  private eventId: number;
  private householdId: number;
  private processing: boolean;
  children: Child[];

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private route: ActivatedRoute,
               private headerService: HeaderService) {}

 ngOnInit() {
   this.processing = false;
   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];

   this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
     this.headerService.announceEvent(event);
   }, error => console.error(error));

   this.adminService.getChildrenByHousehould(+this.eventId, +this.householdId).subscribe((children) => {
     debugger
     this.children = children;
   }, error => console.error(error));
 }

 signIn() {
   console.log('sign in');
   //  if (!this._eventParticipants.hasSelectedParticipants()) {
   //    return this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
   //  }
   //  this.isReady = false;
   //  // remove unselected event participants
   //  this._eventParticipants.removeUnselectedParticipants();
   //  this.childSigninService.signInChildren(this._eventParticipants, this.numberEventsAttending).subscribe(
   //    (response: EventParticipants) => {
   //      this.isReady = true;
   //      if (response && response.Participants && response.Participants.length > 0) {
   //        this.router.navigate(['/child-signin/assignment']);
   //      } else {
   //        this.rootService.announceEvent('generalError');
   //      }
   //    }, (err) => {
   //      this.isReady = true;
   //      this.rootService.announceEvent('generalError');
   //    }
   //  );
 }

}
