import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
@Component({
  selector: 'crds-datepicker',
  templateUrl: 'crds-datepicker.component.html',
  styleUrls: ['crds-datepicker.component.scss']
})
export class CrdsDatePickerComponent implements OnInit {
  @Input() name: string;
  @Input() date: Date;
  @Input() isOpen: boolean = false;
  @Input() autoClose: boolean = false;
  @Output() onChange: EventEmitter<any> = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  public change(newDate: Date) {
    this.date.setDate(newDate.getTime());
    this.onChange.emit(newDate);

    if (this.autoClose) {
      this.toggle();
    }
  }

  public toggle() {
    this.isOpen = !this.isOpen;
  }
}
