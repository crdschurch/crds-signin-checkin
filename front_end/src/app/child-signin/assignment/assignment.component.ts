import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EventParticipants } from '../../shared/models';
import { ChildSigninService } from '../child-signin.service';

@Component({
  selector: 'assignment',
  templateUrl: 'assignment.component.html',
  styleUrls: ['../scss/_cards.scss', ]
})
export class AssignmentComponent implements OnInit {
  // TODO should be childrenResult: Array<any> = [];
  private error: boolean;
  private childrenResult: any;
  private printed: number = 0;
  private printTotal: number;
  constructor(private childSigninService: ChildSigninService, private router: Router) {}

  ngOnInit() {
    const results: any = this.childSigninService.getEventParticipantsResults();
    if (results) {
      this.childrenResult = results.Participants;
      // TODO: call printer here
      // update every 2 seconds for now
      this.printTotal = this.childrenResult.length;
      setInterval(() => { this.printed++; }, 1000);
      // redirect after five seconds
      setTimeout(() => {
        this.router.navigate(['/child-signin/search']);
      }, 5000);
    } else {
      this.error = true;
    }
  }
}
