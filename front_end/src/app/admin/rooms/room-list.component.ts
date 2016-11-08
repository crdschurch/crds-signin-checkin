import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../admin.service';
import { Room } from '../../shared/models';
import { HeaderService } from '../header/header.service';

@Component({
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ ]
})
export class RoomListComponent implements OnInit {
  rooms: Room[];

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private headerService: HeaderService) {}

  private getData(): void {
    const eventId = this.route.snapshot.params['eventId'];
    this.adminService.getRooms(eventId).subscribe(
      (rooms: Room[]) => {
        this.rooms = rooms;
      },
      (error: any) => console.error(error)
    );
    this.adminService.getEvent(eventId).subscribe(
      event => {
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }
}
