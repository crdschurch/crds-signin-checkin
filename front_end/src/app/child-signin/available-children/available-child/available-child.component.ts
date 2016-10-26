import { Component, Input } from '@angular/core';

import { Child } from '../../../shared/models/child';

@Component({
  selector: 'available-childre',
  templateUrl: 'available-child.component.html',
  styleUrls: ['../../scss/_cards.scss', '../../scss/_buttons.scss', ]
})

export class AvailableChildComponent {
  @Input() child: Child;
}
