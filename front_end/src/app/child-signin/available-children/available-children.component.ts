import { Component, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ChildSigninService } from '../child-signin.service';
import { RootService } from '../../shared/services';
import { EventParticipants, Child } from '../../shared/models';

@Component({
  selector: 'available-children',
  templateUrl: 'available-children.component.html',
  styleUrls: [ '../scss/_cards.scss', '../scss/_buttons.scss', 'available-children.component.scss' ]
})

export class AvailableChildrenComponent implements OnInit {
  private eventParticipants: EventParticipants;
  private _isServingOneHour: boolean = false;
  private _isServingTwoHours: boolean = false;
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
        (result) => {
          this.isReady = true;
          this.eventParticipants = result;
        }, (err) => {
          this.isReady = true;
          this.rootService.announceEvent('generalError');
        }
      );
    });
 }

 signIn() {
   if (!this.eventParticipants.hasSelectedParticipants()) {
     this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
     return;
   }

   this.isReady = false;
   this.childSigninService.signInChildren(this.eventParticipants).subscribe(
     (response: EventParticipants) => {
       this.isReady = true;
       if (response && response.Participants && response.Participants.length > 0) {
         this.router.navigate(['/child-signin/assignment']);
       } else {
         this.rootService.announceEvent('generalError');
       }
     }, (err) => {
       this.isReady = true;
       this.rootService.announceEvent('generalError');
     }
   );
 }

 // openShowServiceSelectModal() {
 //   this.showServiceSelectModal();
 // }

 public childrenAvailable(): Child[] {
   return this.eventParticipants.Participants;
 }

 get isServing() {
   return this._isServingOneHour || this._isServingTwoHours;
 }

 get isServingOneHour() {
   return this._isServingOneHour;
 }

 get isServingTwoHours() {
   return this._isServingTwoHours;
 }

 toggleServingHours(hours) {
   if (hours === 1) {
     this._isServingTwoHours = false;
     this._isServingOneHour = !this._isServingOneHour;
   } else if (hours === 2) {
     this._isServingOneHour = false;
     this._isServingTwoHours = !this._isServingTwoHours;
   }
 }

}
