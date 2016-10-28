import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChange } from '@angular/core';

import { Child } from '../../../shared/models/child';

@Component({
  selector: 'available-child',
  templateUrl: 'available-child.component.html',
  styleUrls: ['../../scss/_cards.scss', '../../scss/_buttons.scss', ]
})

export class AvailableChildComponent implements OnInit {
  @Input() child: Child;
  @Output() sharedVarChange = new EventEmitter();
  @Input() sharedVar: string = 'George';

  ngOnInit() {
    console.log(this)
    this.sharedVarChange.emit("innnnnit");
  }

  change() {
    console.log("change");
    this.sharedVarChange.emit("bob");
  }
}
