import { CrdsDatePickerComponent } from './crds-datepicker.component';
import { EventEmitter } from '@angular/core';

describe('CrdsDatePickerComponent', () => {
  let fixture: CrdsDatePickerComponent;
  let dateChange: EventEmitter<Date>;
  let date: Date;
  let isOpen: boolean;
  let autoClose: boolean;
  let onChange: EventEmitter<Date>;

  beforeEach(() => {
    dateChange = jasmine.createSpyObj<EventEmitter<Date>>('dateChange', ['emit']);
    date = new Date();
    isOpen = false;
    autoClose = false;
    onChange = jasmine.createSpyObj<EventEmitter<Date>>('onChange', ['emit']);

    fixture = new CrdsDatePickerComponent();
    fixture.dateChange = dateChange;
    fixture.date = date;
    fixture.isOpen = isOpen;
    fixture.autoClose = autoClose;
    fixture.onChange = onChange;
  });

  describe('#ngOnInit', () => {
    it('should throw an error if no date is specified', () => {
      fixture.date = undefined;
      expect(() => { fixture.ngOnInit(); }).toThrowError(Error, /The "date:Date" attribute must be specified/);
    });

    it('should not throw an error if a date is specified', () => {
      fixture.date = new Date();
      expect(() => { fixture.ngOnInit(); }).not.toThrow();
    });
  });

  describe('#change', () => {
    it('should emit events and toggle the picker if autoClose is true', () => {
      fixture.autoClose = true;
      fixture.isOpen = true;
      date = new Date('1973/10/15');

      fixture.change(date);
      expect(dateChange.emit).toHaveBeenCalledWith(date);
      expect(onChange.emit).toHaveBeenCalledWith(date);
      expect(fixture.isOpen).toBeFalsy();
    });

    it('should emit events but NOT toggle the picker if autoClose is false', () => {
      fixture.autoClose = false;
      fixture.isOpen = true;
      date = new Date('1973/10/15');

      fixture.change(date);
      expect(dateChange.emit).toHaveBeenCalledWith(date);
      expect(onChange.emit).toHaveBeenCalledWith(date);
      expect(fixture.isOpen).toBeTruthy();
    });

  });
});
