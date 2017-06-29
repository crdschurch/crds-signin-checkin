import { Constants } from '../constants';

export class Event {
  static TYPE = {
    ADVENTURE_CLUB: 20
  };
  EventId: number;
  EventTitle: string;
  EventStartDate: string;
  EventTypeId: number;
  EventSite: string;
  Template: boolean;
  Template: boolean;
  EventSiteId: number;
  ParentEventId: number;

  static fromJson(json: any): Event {
    let e = new Event();
    e.EventId = json.EventId;
    e.EventTitle = json.EventTitle;
    e.EventStartDate = json.EventStartDate;
    e.EventTypeId = json.EventTypeId;
    e.EventSite = json.EventSite;
    e.IsCurrentEvent = json.IsCurrentEvent;
    e.Template = json.Template;
    e.EventSiteId = json.EventSiteId;
    e.ParentEventId = json.ParentEventId;
    return e;
  }

  static fromJsons(jsons: any): Event[] {
    let events: Event[] = [];
    for (let json of jsons) {
        events.push(Event.fromJson(json));
    }
    return events;
  }

  public isAdventureClub() {
    return this.EventTypeId === Event.TYPE.ADVENTURE_CLUB;
  }

  get isStudentMinistry() {
    return this.EventTypeId === Constants.BigEventType || this.EventTypeId === Constants.StudentMinistry6through8EventType ||
      this.EventTypeId === Constants.StudentMinistry9through12EventType;
  }

  get isChildCare() {
    return this.EventTypeId === Constants.ChildCareEventType;
  }

  get type() {
    switch (this.EventTypeId) {
      case Constants.StudentMinistry:
        return 'Student Ministry';
      case Constants.ChildCareEventType:
        return 'Childcare';
      case Constants.BigEventType:
        return 'Student Ministry';
      case Constants.StudentMinistry6through8EventType:
        return 'Student Ministry';
      case Constants.StudentMinistry9through12EventType:
        return 'Student Ministry';
      default:
        return 'Service';
    }
  }
}
