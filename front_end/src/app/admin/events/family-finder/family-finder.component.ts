// tslint:disable:no-unused-variable

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { AdminService } from '../../admin.service';

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
    private adminService: AdminService) {}

  ngOnInit() {
    this.processing = false;
    this.eventId = this.route.snapshot.params['eventId'];
  }
}
