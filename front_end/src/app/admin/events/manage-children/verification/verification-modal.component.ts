import { Component, Output, Input, ViewChild, EventEmitter, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { Child } from '../../../../shared/models';

@Component({
  selector: 'verification-modal',
  templateUrl: 'verification-modal.component.html',
  styleUrls: [ 'verification-modal.component.scss' ]
})
export class VerificationModalComponent implements OnInit {
  @ViewChild('verificationModal') public verificationModal: ModalDirective;
  @Output() reprint = new EventEmitter<any>();
  @Input() child: Child;
  showVerificationOption = false;
  showModal = false;
  parent1: any;
  parent2: any;

  constructor() {
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

  print(modal) {
    this.reprint.emit(this.child);
    modal.hide();
  }

  setupParents() {
    if (this.child.HeadsOfHousehold !== undefined) {
      this.parent1 = this.child.HeadsOfHousehold[0];
      this.parent2 = this.child.HeadsOfHousehold[3];
    }
  }
}
