// tslint:disable:no-unused-variable

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Contact } from '../../../shared/models'
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
  private searched: boolean;
  private search: string;
  private contacts: Array<Contact> = [];

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private headerService: HeaderService,
    private apiService: ApiService) {}

  ngOnInit() {
    this.processing = false;
    this.searched = false;
    this.eventId = this.route.snapshot.params['eventId'];
    
    this.apiService.getEvent(this.eventId).subscribe((event) => {
      this.headerService.announceEvent(event);
    }, error => console.error(error));
  }

  setSearchValue(search) {
    this.search = search;
  }

  onClearSearch(box) {
    this.search = '';
    box.value = '';
    this.executeSearch();
  }

  onSearch() {
    this.executeSearch();
  }

  private executeSearch() {
    this.searched = true;
    this.processing = true;

  }
}
