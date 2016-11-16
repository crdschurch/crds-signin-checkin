import { Component, OnInit } from '@angular/core';
import { EventParticipants } from '../../shared/models';
import { ChildSigninService } from '../child-signin.service';

@Component({
  selector: 'assignment',
  templateUrl: 'assignment.component.html',
  styleUrls: ['../scss/_cards.scss', ]
})
export class AssignmentComponent  implements OnInit {
  // TODO should be childrenResult: Array<any> = [];
  private childrenResult: any;
  constructor(private childSigninService: ChildSigninService) {}

  ngOnInit() {
    const results: any = this.childSigninService.getEventParticipantsResults();
    if (results) {
      this.childrenResult = results.Participants;
    }
  }
}
