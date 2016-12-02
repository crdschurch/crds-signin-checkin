import { NewChildComponent } from './new-child.component';
import { NewChild } from '../../../../shared/models';
import * as moment from 'moment';

describe('NewChildComponent', () => {
  it('should return true when child > 5 years old', () => {
    let fixture = new NewChildComponent();
    let child = new NewChild();
    child.DateOfBirth = moment().subtract(5, 'years').subtract(1, 'day').toDate();
    expect(fixture.needGradeLevel(child)).toBeTruthy();
  });

  it('should return false when child < 5 years old', () => {
    let fixture = new NewChildComponent();
    let child = new NewChild();
    child.DateOfBirth = moment().subtract(5, 'years').add(1, 'day').toDate();
    expect(fixture.needGradeLevel(child)).toBeFalsy();
  });
});
