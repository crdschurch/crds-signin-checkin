import { Component, OnInit, Input } from '@angular/core';

import { NewChild } from '../../../../shared/models';

import * as moment from 'moment';

@Component({
  selector: 'new-child',
  templateUrl: 'new-child.component.html'
})
export class NewChildComponent implements OnInit {
  @Input() childNumber: number;
  @Input() child: NewChild;

  constructor() {}

  ngOnInit() {
  }

  needGradeLevel(child: NewChild): boolean {
    return moment(child.DateOfBirth).isBefore(moment().startOf('day').subtract(5, 'y'));
  }
}
