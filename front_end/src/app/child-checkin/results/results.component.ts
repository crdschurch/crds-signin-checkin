import { Component, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

@Component({
  selector: 'results',
  templateUrl: 'results.component.html',
  styleUrls: ['../scss/_cards.scss', '../scss/_buttons.scss', ]
})

export class ResultsComponent {

  private cb1: boolean = true;
  private cb2: boolean = true;
  private cb3: boolean = false;
  private cb4: boolean = true;
  private cb5: boolean = true;
  private cb6: boolean = true;
  private cb7: boolean = true;
  private cb8: boolean = true;
  private cb9: boolean = true;
  private cb10: boolean = true;

  private serving1: boolean = true;

 @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;

  public showServiceSelectModal(): void {
    this.serviceSelectModal.show();
  }
}
