import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'loading-button',
  templateUrl: 'loading-button.component.html'
})
export class LoadingButtonComponent implements OnInit {
  @Input() buttonType: string;
  @Input() buttonClasses: string;
  @Input() loading: any;
  @Input() normalText: string;
  @Input() loadingText: string;
  @Input() loadingClass: string;

  constructor() { }

  ngOnInit() { }

  buttonClass() {
    return this.loading ? this.loadingClass : '';
  }

  buttonText() {
    return this.loading ? this.loadingText : this.normalText;
  }
}
