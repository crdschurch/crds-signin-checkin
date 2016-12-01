import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { NewParent } from '../../../../shared/models';

@Component({
  selector: 'new-parent',
  templateUrl: 'new-parent.component.html'
})
export class NewParentComponent implements OnInit {
  @Input() parent: NewParent;
  @Output() updateNumberOfKids: EventEmitter<any> = new EventEmitter();
  private numberOfPossibleKids: Array<number> = [];

  constructor() {}

  ngOnInit() {
    this.numberOfPossibleKids = Array.from({length: 12}, (v, k) => k + 1);
  }

  update(numberOfKids) {
    this.updateNumberOfKids.emit(numberOfKids);
  }
}
