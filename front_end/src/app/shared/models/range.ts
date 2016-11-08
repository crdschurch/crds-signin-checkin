export class Range {
  Id: number;
  Name: string;
  Selected: boolean;
  SortOrder: number;
  TypeId: number;

  static fromJson(json: any): Range {
    if (!json) {
      return new Range();
    }

    let range = new Range();
    range.Id = json.Id;
    range.Name = json.Name;
    range.Selected = json.Selected;
    range.SortOrder = json.SortOrder;
    range.TypeId = json.TypeId;
    return range;
  }
}
