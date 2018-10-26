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
  }
}
