import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
@Component({
  selector: 'crds-datepicker',
  templateUrl: 'crds-datepicker.component.html',
  styleUrls: ['crds-datepicker.component.scss']
})
export class CrdsDatePickerComponent implements OnInit {
  @Input() name: string;
  @Input() date: Date;
  @Output() dateChange: EventEmitter<Date> = new EventEmitter<Date>();
  @Input() isOpen: boolean = false;
  @Input() autoClose: boolean = false;
  @Output() onChange: EventEmitter<any> = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
    if (!this.date) {
      throw new Error('The "date:Date" attribute must be specified. This is the model representing the date selected by the datepicker');
    }
  }

  public change(newDate: Date) {
    this.dateChange.emit(newDate);
    this.onChange.emit(newDate);

    if (this.autoClose) {
      this.toggle();
    }
  }

  public toggle() {
    this.isOpen = !this.isOpen;
  }
}
