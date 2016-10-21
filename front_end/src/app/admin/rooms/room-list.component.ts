import { Component } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ AdminService ]
})
export class RoomListComponent {
  rooms: Room[];

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private service: AdminService) {}

  private getData(): void {
    const eventId = this.route.snapshot.params['eventId'];
    this.adminService.getRooms(eventId).subscribe(
      (rooms: Room[]) => {this.rooms = rooms},
      (error: any) => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }
}
