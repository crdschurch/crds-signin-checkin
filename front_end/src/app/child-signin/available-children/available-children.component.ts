import { Component, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { ChildSigninService } from '../child-signin.service';
import { Child } from '../../shared/models/child';

@Component({
  selector: 'available-children',
  templateUrl: 'available-children.component.html',
  styleUrls: ['../scss/_cards.scss', '../scss/_buttons.scss', ]
})

export class AvailableChildrenComponent implements OnInit {
  private childrenAvailable: Array<Child> = [];
  private isServing: boolean = true;

 @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;

 constructor(private childSigninService: ChildSigninService, private route: ActivatedRoute) { }

 ngOnInit() {
   this.route.params.forEach((params: Params) => {
      let phoneNumber = params['phoneNumber'];

      this.childSigninService.getChildrenByPhoneNumber(phoneNumber).
        subscribe(childrenAvailable => this.childrenAvailable = childrenAvailable);
    });
 }

 signIn() {
   console.log(this.childrenAvailable);
 }

 public showServiceSelectModal(): void {
   this.serviceSelectModal.show();
  }
}
