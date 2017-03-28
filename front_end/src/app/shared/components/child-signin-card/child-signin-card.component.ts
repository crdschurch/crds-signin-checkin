import { Component, Input } from '@angular/core';

import { Child, NewChild } from '../../models';

@Component({
  selector: 'child-signin-card',
  templateUrl: 'child-signin-card.component.html',
  // styleUrls: ['./_cards.scss', '../../../../assets/scss/_buttons.scss', ]
  // styleUrls: ['./_cards.scss']
})

export class ChildSigninCardComponent {
  @Input() child: Child|NewChild;
}
