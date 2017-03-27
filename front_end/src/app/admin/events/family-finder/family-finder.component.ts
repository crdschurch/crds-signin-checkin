// tslint:disable:no-unused-variable

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { AdminService } from '../../admin.service';
import { HeaderService } from '../../header/header.service';
import { ApiService } from '../../../shared/services';

@Component({
  styleUrls: ['family-finder.component.scss'],
  selector: 'family-finder',
  templateUrl: 'family-finder.component.html'
})
export class FamilyFinderComponent implements OnInit {
  private eventId: string;
  private processing: boolean;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private headerService: HeaderService,
    private apiService: ApiService) {}

  ngOnInit() {
    this.processing = false;
    this.eventId = this.route.snapshot.params['eventId'];

   this.apiService.getEvent(this.eventId).subscribe((event) => {
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );
  }
}
