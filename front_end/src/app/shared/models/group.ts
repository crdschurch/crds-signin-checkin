import { Range } from '.';

export class Group {
  Id: number;
  Name: string;
  Selected: boolean;
  SortOrder: number;
  TypeId: number;
  Group_Name: string;
  Ranges: Range[];

  static fromJson(json: any): Group {
    if (!json) {
      return new Group();
    }

    let group = new Group();
    group.Id = json.Id;
    group.Name = json.Name;
    group.Selected = json.Selected;
    group.SortOrder = json.SortOrder;
    group.TypeId = json.TypeId;
    group.Group_Name = json.Group_Name;
    group.Ranges = json.Ranges !== undefined && json.Ranges !== null && json.Ranges.length !== 0
                    ? json.Ranges.map((r) => Range.fromJson(r))
                    : [];
    return group;
  }

  static fromJsons(jsons: any): Group[] {
    let groups: Group[] = [];
    for (let json of jsons) {
        groups.push(Group.fromJson(json));
    }
    return groups;
  }


  getSelectionDescription(): string {
    if (this.Ranges === undefined || this.Ranges.length === 0) {
      if (this.Selected) {
        return `${this.Name}: All`;
      } else {
        return null;
      }
    }

    let selectedRanges = this.Ranges.filter((r) => { return r.Selected; });
    if (selectedRanges.length === 0) {
      return null;
    }

    if (selectedRanges.length === this.Ranges.length) {
      return `${this.Name}: All`;
    } else {
      return `${this.Name}: ${selectedRanges.map((r) => { return r.Name; }).join(', ')}`;
    }
  }
}
