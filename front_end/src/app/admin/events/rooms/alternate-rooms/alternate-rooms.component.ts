import { Component, Output, Input, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Room } from '../../../../shared/models';
import { AdminService } from '../../../admin.service';
import * as _ from 'lodash';

@Component({
  selector: 'alternate-rooms',
  templateUrl: 'alternate-rooms.component.html',
  styleUrls: ['alternate-rooms.component.scss']
})
export class AlternateRoomsComponent implements OnInit {
  @Input() allAlternateRooms: Room[];
  @Output() setDirty = new EventEmitter<boolean>();
  @Output() setBumpingType = new EventEmitter<number>();
  _selectedBumpingType: number;

  constructor( private adminService: AdminService,
               private route: ActivatedRoute) {}

  setDirtyChild() {
    this.setDirty.emit(true);
  }

  get selectedBumpingType() {
    return this._selectedBumpingType;
  }

  set selectedBumpingType(p) {
    this._selectedBumpingType = p;
    this.setBumpingType.emit(p);
  }

  get allRooms() {
    return this.allAlternateRooms;
  }

  set allRooms(rooms) {
    this.allAlternateRooms = rooms;
  }

  get availableRooms(): Room[] {
    if (this.allAlternateRooms) {
      return this.allAlternateRooms.filter( (obj: Room) => { return !obj.isBumpingRoom(); } );
    };
  }

  get bumpingRooms(): Room[] {
    if (this.allAlternateRooms) {
      return this.allAlternateRooms
        .filter(
          (obj: Room) => { return obj.isBumpingRoom(); })
        .sort(
          (a, b) => {
            if (a.BumpingRulePriority > b.BumpingRulePriority) {
              return 1;
            } else if (a.BumpingRulePriority < b.BumpingRulePriority) {
              return -1;
            } else {
              return 0;
            }
          }
        );
    }
  }

  isTypePriority() {
    return this.selectedBumpingType === Room.BUMPING_TYPE.PRIORITY;
  }

  isTypeVacancy() {
    return this.selectedBumpingType === Room.BUMPING_TYPE.VACANCY;
  }

  ngOnInit() {
    if (_.some(this.allAlternateRooms, { 'BumpingRuleTypeId': Room.BUMPING_TYPE.VACANCY })) {
        this.selectedBumpingType = Room.BUMPING_TYPE.VACANCY;
    } else {
      this.selectedBumpingType = Room.BUMPING_TYPE.PRIORITY;
    }
  }

  selectTypePriority() {
    this.selectedBumpingType = Room.BUMPING_TYPE.PRIORITY;
    this.setDirtyChild();
  }

  selectTypeVacancy() {
    this.selectedBumpingType = Room.BUMPING_TYPE.VACANCY;
    this.setDirtyChild();
  }

}
