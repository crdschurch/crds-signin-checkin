import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { AdminService } from '../../admin/admin.service';
import { Event } from '../../admin/events/event';
import * as moment from 'moment';

@Component({
  selector: 'room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss' ]
})

export class RoomComponent implements OnInit {
  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
  @ViewChild('childDetailModal') public childDetailModal: ModalDirective;
  todaysEvents: Event[];

  constructor(private adminService: AdminService) { }

  ngOnInit() {
    let today = new Date();
    this.adminService.getEvents(today, today).subscribe(
      events => {
        this.todaysEvents = events;
      },
      error => { console.error(error); }
    );
  }

  public showServiceSelectModal() {
    this.serviceSelectModal.show();
  }

  public showChildDetailModal() {
    this.childDetailModal.show();
  }
}
