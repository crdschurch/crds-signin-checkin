import { Component, Input } from '@angular/core';

import { Child, NewChild } from '../../models';

@Component({
  selector: 'child-signin-card',
  templateUrl: 'child-signin-card.component.html'
})

export class ChildSigninCardComponent {
  @Input() child: Child|NewChild;
}
