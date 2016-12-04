import { Component, Input } from '@angular/core';

import { NewChild, Group } from '../../../../shared/models';

import * as moment from 'moment';

@Component({
  styleUrls: ['new-child.component.scss'],
  selector: 'new-child',
  templateUrl: 'new-child.component.html'
})
export class NewChildComponent {
  @Input() submitted: boolean;
  @Input() childNumber: number;
  @Input() child: NewChild;
  @Input() gradeGroups: Array<Group>;

  constructor() {}

  needGradeLevel(child: NewChild): boolean {
    return moment(child.DateOfBirth).isBefore(moment().startOf('day').subtract(5, 'y'));
  }

  update(groupId: number) {
    this.child.YearGrade = groupId;
  }
}
