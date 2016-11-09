import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'oneBasedIndex'
})

export class OneBasedIndexPipe implements PipeTransform {
  transform(val: number) {
    return ++val;
  }
}
