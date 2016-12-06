import { NewParent, NewChild, Event } from '.';

export class NewFamily {
  event: Event;
  parent: NewParent;
  children: Array<NewChild> = [];
  numberOfKids: number = 1;

  static fromJson(json: any): NewFamily {
    if (!json) {
      return new NewFamily();
    }

    let newFamily = new NewFamily();
    newFamily.event = Event.fromJson(json.event);
    newFamily.parent = NewParent.fromJson(json.parent);

    newFamily.children = [];
    for (let p of json.children) {
      newFamily.children.push(NewChild.fromJson(p));
    }

    return newFamily;
  }
}
