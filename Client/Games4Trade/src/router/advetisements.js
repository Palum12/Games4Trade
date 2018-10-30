import ShowAdvertisement from '../views/advertisements/ShowAdvertisement'
import CreateOrUpdateAdvertisement from '../views/advertisements/CreateOrUpdateAdvertisement'
import SearchAdvertisements from '../views/advertisements/SearchAdvertisements'
export default {
  ADDADVERTISEMENT: {
    path: '/advertisements/add',
    name: 'AddAdvertisement',
    component: CreateOrUpdateAdvertisement
  },
  EDITADVERTISEMENT: {
    path: '/advertisements/:id/edit',
    name: 'EditAvertisement',
    component: CreateOrUpdateAdvertisement
  },
  SHOWADVERTISEMENT: {
    path: '/advertisements/:id',
    name: 'ShowAvertisement',
    component: ShowAdvertisement
  },
  SEARCHADVERTISEMENTS: {
    path: '/advertisements/search/:text',
    name: 'SearchAdvertisement',
    component: SearchAdvertisements
  }
}
