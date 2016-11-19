import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Child, EventParticipants } from '../../shared/models';
import { ChildSigninService } from '../child-signin.service';
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'assignment',
  templateUrl: 'assignment.component.html',
  styleUrls: ['../scss/_cards.scss']
})
export class AssignmentComponent implements OnInit {
  private error: boolean;
  private childrenResult: Array<Child>;
  private printed: number = 0;
  private printTotal: number;
  constructor(private childSigninService: ChildSigninService, private router: Router) { }

  ngOnInit() {
    const results = this.childSigninService.getEventParticipantsResults();
    if (results) {
      this.childrenResult = results.Participants;
      let progressBar = this.startProgressBar(this.childrenResult);

      this.childSigninService.printParticipantLabels(results).subscribe((eventParticipants: EventParticipants) => {
        progressBar.unsubscribe();
        this.router.navigate(['/child-signin/search']);
      }, (error) => {
        progressBar.unsubscribe();
        this.error = true;
      });
    } else {
      this.error = true;
    }
  }

  private startProgressBar(children: Child[]): Subscription {
    // TODO This could eventually call to get status of print job (or use websockets)
    this.printTotal = 10;
    return Observable.interval(250).subscribe((next) => {
      this.printed = (next % this.printTotal) + 1;
    });
  }
}
