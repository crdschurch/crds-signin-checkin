export class Event {
  EventId: number;
  EventTitle: string;
  EventStartDate: string;
  EventType: string;
  EventSite: string;
  EventSiteId: number;

  static fromJson(json: any): Event {
    let e = new Event();
    e.EventId = json.EventId;
    e.EventTitle = json.EventTitle;
    e.EventStartDate = json.EventStartDate;
    e.EventType = json.EventType;
    e.EventSite = json.EventSite;
    e.EventSiteId = json.EventSiteId;
    return e;
  }

  constructor() {
  }
}
