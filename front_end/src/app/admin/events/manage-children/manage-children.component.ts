import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Event } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';
@Component({
  templateUrl: 'manage-children.component.html',
  styleUrls: ['manage-children.component.scss'],
  providers: [ ],
})
export class ManageChildrenComponent implements OnInit {
  ready: boolean = true;

  constructor(private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private rootService: RootService,
    private adminService: AdminService,
    private router: Router) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];
    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.headerService.announceEvent(event);
    });
  }

  public isReady(): boolean {
    return this.ready;
  }
}
