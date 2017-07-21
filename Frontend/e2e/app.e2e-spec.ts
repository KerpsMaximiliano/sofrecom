import { SofcoNoMaterialPage } from './app.po';

describe('sofco-no-material App', () => {
  let page: SofcoNoMaterialPage;

  beforeEach(() => {
    page = new SofcoNoMaterialPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
