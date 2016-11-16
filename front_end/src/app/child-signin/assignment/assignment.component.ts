import { Component, OnInit } from '@angular/core';
import { EventParticipants } from '../../shared/models';
import { ChildSigninService } from '../child-signin.service';

@Component({
  selector: 'assignment',
  templateUrl: 'assignment.component.html',
  styleUrls: ['../scss/_cards.scss', ]
})
export class AssignmentComponent  implements OnInit {
  private childrenResult: Array<EventParticipants> = [];
  constructor(private childSigninService: ChildSigninService) {}

  ngOnInit() {
    this.childrenResult = this.childSigninService.getEventParticipantsResults();
  }
}
