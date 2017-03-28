import { Component, Input } from '@angular/core';

import { Child, NewChild } from '../../models';

@Component({
  selector: 'child-signin',
  templateUrl: 'child-signin.component.html',
  // styleUrls: ['../../../scss/_cards.scss', '../../../scss/_buttons.scss', ]
})

export class ChildSigninComponent {
  @Input() child: Child|NewChild;
}
