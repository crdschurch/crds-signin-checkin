export class Event {
  EventId: number;
  EventTitle: string;
  EventStartDate: string;
  EventType: string;
  EventSite: string;
  IsCurrentEvent: boolean;

  static fromJson(json: any): Event {
    let e = new Event();
    e.EventId = json.EventId;
    e.EventTitle = json.EventTitle;
    e.EventStartDate = json.EventStartDate;
    e.EventType = json.EventType;
    e.EventSite = json.EventSite;
    e.IsCurrentEvent = json.IsCurrentEvent;
    return e;
  }

  constructor() {
  }
}
