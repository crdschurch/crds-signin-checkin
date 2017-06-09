import { NewParent, NewChild, Event } from '.';

export class NewFamily {
  event: Event;
  parents: Array<NewParent> = [];
  children: Array<NewChild> = [];
  numberOfKids = 1;
  numberOfParents = 1;

  static fromJson(json: any): NewFamily {
    if (!json) {
      return new NewFamily();
    }

    let newFamily = new NewFamily();
    newFamily.event = Event.fromJson(json.event);

    newFamily.parents = [];
    for (let p of json.parents) {
      newFamily.parents.push(NewParent.fromJson(p));
    }

    newFamily.children = [];
    for (let p of json.children) {
      newFamily.children.push(NewChild.fromJson(p));
    }

    return newFamily;
  }

  allChildrenHaveBirthdays() {
    for (let c of this.children) {
      if (!c.DateOfBirth) {
        return false;
      }
    }
    return true;
  }
}
