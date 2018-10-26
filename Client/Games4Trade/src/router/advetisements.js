import ShowAdvertisement from '../views/advertisements/ShowAdvertisement'
import CreateOrUpdateAdvertisement from '../views/advertisements/CreateOrUpdateAdvertisement'
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
  }
}
