import { Component, Input } from '@angular/core';

import { Child, NewChild } from '../../models';

@Component({
  selector: 'available-child',
  templateUrl: 'available-child.component.html',
  // styleUrls: ['../../../scss/_cards.scss', '../../../scss/_buttons.scss', ]
})

export class AvailableChildComponent {
  @Input() child: Child|NewChild;
}
