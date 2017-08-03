import { Component, Output, Input, ViewChild, EventEmitter, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { Child } from '../../../../shared/models';
import { AdminService } from '../../../admin.service';

@Component({
  selector: 'verification-modal',
  templateUrl: 'verification-modal.component.html',
  styleUrls: [ 'verification-modal.component.scss' ]
})
export class VerificationModalComponent implements OnInit {
  @ViewChild('verificationModal') public verificationModal: ModalDirective;
  @Input() child: Child;
  showVerificationOption = false;
  showModal = false;
  parent1: any;
  parent2: any;

  constructor(private adminService: AdminService) {
  }

  ngOnInit() {
    this.showVerificationOption = true;
  }

  showVerificationModal(modal) {
    if (modal) {
      this.setupParents();
      this.showModal = true;
      modal.show();
    }
  }

  setupParents() {
    if (this.child.HeadsOfHousehold !== undefined) {
      this.parent1 = this.child.HeadsOfHousehold[0];
      this.parent2 = this.child.HeadsOfHousehold[3];
    }
  }

  reprint(child: Child) {
    /*
    this.ready = false;

    this.adminService.reprint(child.EventParticipantId).subscribe((resp) => {
      this.ready = true;
    },
    (error) => {
      switch (error.status) {
        case 412:
          this.rootService.announceEvent('echeckNewFamilyAdminSetupWrong');
          break;
        default:
          this.rootService.announceEvent('generalError');
          break;
        }
      this.ready = true;
    });
    */
  }
}
