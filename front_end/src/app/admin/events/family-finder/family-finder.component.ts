// tslint:disable:no-unused-variable
/* tslint:disable:max-line-length */

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Contact } from '../../../shared/models';
import { AdminService } from '../../admin.service';
const phoneRegex = /^\d{3}-\d{3}-\d{4}$/;
const nameRegex = /^[a-zA-Z]*$/;
const emailRegex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

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
    private router: Router) {}

  ngOnInit() {
    this.processing = false;
    this.searched = false;
    this.eventId = this.route.snapshot.params['eventId'];

    this.search = (this.route.snapshot.queryParams['search'] || '').trim();
    if (this.search.length > 0) {
      this.executeSearch();
    }
  }

  getSearchParams() {
    if (!this.search || !this.search.length) {
      return;
    }
    this.search.trim();
    let parentParams = {
      first: '',
      last: '',
      phone: '',
      email: ''
    };

    if (this.search.indexOf(',') > -1) {
      parentParams.last = this.search.split(',')[0].trim();
      parentParams.first = this.search.split(',')[1].trim();
    } else if (phoneRegex.test(this.search)) {
      parentParams.phone = this.search;
    } else if (nameRegex.test(this.search)) {
      parentParams.last = this.search;
    } else if (emailRegex.test(this.search)) {
      parentParams.email = this.search;
    }
    return parentParams;
  }

  onClearSearch(box) {
    this.search = '';
    this.contacts = [];
    this.searched = false;
  }

  onSearch() {
    this.router.navigate(['/admin/events', this.eventId, 'family-finder'], {queryParams: {search: this.search}});
    this.executeSearch();
  }

  private executeSearch() {
    this.searched = true;
    this.processing = true;

    this.adminService.findFamilies(this.search).subscribe((contacts: Array<Contact>) => {
      this.contacts = contacts;
      this.processing = false;
    }, error => console.error(error));
  }
}
