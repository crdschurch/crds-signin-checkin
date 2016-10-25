export class Range {
  Id: number;
  Name: string;
  Selected: boolean;
  SortOrder: number;
  TypeId: number;

  toggleSelected() {
    this.Selected = !this.Selected;
  }
}
