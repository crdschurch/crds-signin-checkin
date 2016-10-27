import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Http } from '@angular/http';
import { Toast, BodyOutputType } from 'angular2-toaster/angular2-toaster';
import { ContentBlock } from '../models/contentBlock';

@Injectable()
export class ContentService {

  private contentBlockTitle: string;
  contentBlocks: ContentBlock[];

  constructor(private http: Http) {}

  loadData() {
    // call for each type of content block used in the app
    this.getContentBlocks('main').then(mainContentBlocks => {
      this.contentBlocks = mainContentBlocks;

      this.getContentBlocks('common').then(commonContentBlocks => {
        this.contentBlocks = this.contentBlocks.concat(commonContentBlocks);
      });

    });
  }

  getContentBlocks (categoryName) {
      const url = `${process.env.CRDS_CMS_ENDPOINT}/api/contentblock?category=` + categoryName;
      return this.http.get(url).toPromise()
                    .then(res => { return res.json().contentblocks; })
                    .catch(this.handleError);
  }

  getToastContent (contentBlockTitle): Promise<any> {
    this.contentBlockTitle = contentBlockTitle;
    if (this.contentBlocks === undefined) {
      return this.getContentBlocks('main').then(mainContentBlocks => {
        this.contentBlocks = mainContentBlocks;

        return this.getContentBlocks('common').then(commonContentBlocks => {
          this.contentBlocks = this.contentBlocks.concat(commonContentBlocks);
          return this.displayToast(this.contentBlockTitle);
        });

      });
    } else {
      return Promise.resolve(this.displayToast(this.contentBlockTitle));
    }
  }

  displayToast(contentBlockTitle) {
      const contentBlock = this.contentBlocks.find(x => x.title === contentBlockTitle);

      const toast: Toast = {
        type: contentBlock.type,
        body: contentBlock.content,
        bodyOutputType: BodyOutputType.TrustedHtml
      };

    return toast;
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }

}