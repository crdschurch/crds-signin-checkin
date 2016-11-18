import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Child, EventParticipants } from '../../shared/models';
import { ChildSigninService } from '../child-signin.service';

@Component({
  selector: 'assignment',
  templateUrl: 'assignment.component.html',
  styleUrls: ['../scss/_cards.scss', ]
})
export class AssignmentComponent implements OnInit {
  private error: boolean;
  private childrenResult: Array<Child>;
  private printed: number = 0;
  private printTotal: number;
  constructor(private childSigninService: ChildSigninService, private router: Router) {}

  ngOnInit() {
    const results: any = this.childSigninService.getEventParticipantsResults();
    if (results) {
      this.childrenResult = results.Participants;
      this.childSigninService.printParticipantLabels(results).subscribe((eventParticipants: EventParticipants) => {
        this.printTotal = eventParticipants.Participants.filter(p => p.Selected).length;
        setInterval(() => {
          this.printed++;
          if (this.printed === this.printTotal) {
            // redirect after a second when complete
            setTimeout(() => { this.router.navigate(['/child-signin/search']); }, 1000);
          }
        }, 2000);
      });
    } else {
      this.error = true;
    }
  }
}
