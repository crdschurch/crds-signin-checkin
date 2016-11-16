import { Component, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ChildSigninService } from '../child-signin.service';
import { RootService } from '../../shared/services';
import { EventParticipants, Child } from '../../shared/models';

@Component({
  selector: 'available-children',
  templateUrl: 'available-children.component.html',
  styleUrls: [ '../scss/_cards.scss', '../scss/_buttons.scss' ]
})

export class AvailableChildrenComponent implements OnInit {
  private childrenAvailable: Array<Child> = [];
  private isServing: boolean = false;
  private isReady: boolean = false;

 @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;

 constructor( private childSigninService: ChildSigninService,
              private route: ActivatedRoute,
              private router: Router,
              private rootService: RootService) { }

 ngOnInit() {
   this.route.params.forEach((params: Params) => {
      let phoneNumber = params['phoneNumber'];

      this.childSigninService.getChildrenByPhoneNumber(phoneNumber).subscribe(
        (childrenAvailable) => {
          this.isReady = true;
          this.childrenAvailable = childrenAvailable;
        }, (err) => {
          this.isReady = true;
          this.rootService.announceEvent('generalError');
        }
      );
    });
 }

 signIn() {
   const event = this.childSigninService.getEvent();
   const children = this.childrenAvailable;
   const eventParticipants = EventParticipants.fromJson({ CurrentEvent: event, Participants: children });
   if (!eventParticipants.hasSelectedParticipants()) {
     this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
     return;
   }

   this.isReady = false;
   this.childSigninService.signInChildren(eventParticipants).subscribe((response: EventParticipants) => {
     this.isReady = true;
     if (response && response.Participants && response.Participants.length > 0) {
       this.router.navigate(['/child-signin/assignment']);
     } else {
       this.rootService.announceEvent('generalError');
     }
   });
 }

 public showServiceSelectModal(): void {
   this.serviceSelectModal.show();
  }
}
