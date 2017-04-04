import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService, RootService } from '../../../../shared/services';
import { ChildSigninService } from '../../../../child-signin/child-signin.service';
import { AdminService } from '../../../admin.service';
import { Child, EventParticipants } from '../../../../shared/models';
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
  eventParticipants: EventParticipants;

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private childSigninService: ChildSigninService,
               private rootService: RootService,
               private route: ActivatedRoute,
               private router: Router,
               private headerService: HeaderService) {}

 ngOnInit() {
   this.processing = true;
   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];

   this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
     this.headerService.announceEvent(event);
   }, error => console.error(error));

   this.adminService.getChildrenByHousehold(+this.householdId).subscribe((ep: EventParticipants) => {
     this.eventParticipants = ep;
     if (this.eventParticipants === undefined || !this.eventParticipants.hasParticipants()) {
       this.rootService.announceEvent('echeckFamilyFinderNoChildren');
     }
     this.processing = false;
   }, (err) => {
     if (err === 'No current events for site') {
       this.rootService.announceEvent('noCurrentEvent');
     } else {
       this.rootService.announceEvent('generalError');
     }
     this.processing = false;
   });
 }

 signIn() {
    if (!this.eventParticipants.hasSelectedParticipants()) {
      return this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
    }
    this.processing = true;
    // remove unselected event participants
    this.eventParticipants.removeUnselectedParticipants();
    const numberEventsAttending = 1;
    this.adminService.findFamilySigninAndPrint(this.eventParticipants, numberEventsAttending).subscribe(
      (response: EventParticipants) => {
        this.processing = false;
        if (response && response.Participants && response.Participants.length > 0) {
          this.router.navigate([`/admin/events/${this.eventId}/family-finder`]);
          this.rootService.announceEvent('echeckFamilyFinderSignedIn');
        } else {
          this.rootService.announceEvent('generalError');
        }
      }, (err) => {
        this.rootService.announceEvent('generalError');
        this.processing = false;
      }
    );
 }

}
