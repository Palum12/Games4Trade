<template>
    <div class="no-gutters ">
        <div id="login-form" class="row no-gutters p-2">
            <div class="form rounded col-12 p-3">
                <form @submit.prevent="onSubmit">
                    <div class="form-group">
                        <label for="title">Tytuł</label>
                        <input
                                type="text"
                                id="title"
                                class="form-control"
                                v-model="announcement.title">
                    </div>
                    <div class="form-group">
                        <label for="content">Treść</label>
                        <textarea
                                id="content"
                                class="form-control"
                                rows="20"
                                v-model="announcement.content">
                        </textarea>
                    </div>
                    <div v-if="isEditing" class="form-group d-flex justify-content-end">
                        <div v-if="!announcement.isActive">
                            <button type="button" class="btn btn-info btn-primary btn-block" @click="changeStatus">Udostępnij</button>
                        </div>
                        <div v-if="announcement.isActive">
                            <button type="button" class="btn btn-info btn-warning btn-block" @click="changeStatus">Archiwizuj</button>
                        </div>
                        <div class="ml-2">
                            <button type="button" class="btn btn-info btn-block" @click="goBack">Powrót</button>
                        </div>
                        <div class="ml-2">
                            <button type="button" class="btn btn-info btn-block" @click="modyfi">Modyfikuj</button>
                        </div>
                        <div class="ml-2">
                            <button type="button" class="btn btn-danger btn-block" @click="remove">Usuń</button>
                        </div>
                    </div>
                    <div v-if="!isEditing" class="form-group">
                        <button type="submit" class="btn btn-info btn-block">Dodaj</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'CreateOrUpdateAnnouncement',
  data () {
    return {
      isEditing: false,
      announcement: {
        id: 0,
        title: '',
        content: '',
        author: '',
        dateCreated: '',
        isActive: ''
      }
    }
  },
  methods: {
    onSubmit () {
      let vm = this
      axios.post('announcements', {title: this.announcement.title, content: this.announcement.content})
        .then(() => {
          mixins.methods.customSuccessPopUp(vm, 'Ogłoszenie zostało dodane, ' +
            'ale jeżeli chcesz, żeby pokazało się ono innym użytkownikom ' +
            'to kliknij przycisk "Udostępnij" w liście ogłoszeń')
          vm.$router.go(-1)
        })
        .catch(() => {
          mixins.methods.errorPopUp(vm)
        })
    },
    changeStatus () {
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          vm.$store.dispatch('setSpinnerLoading')
          axios.patch(`announcements/${vm.announcement.id}`, {isActive: !vm.announcement.isActive})
            .then(() => {
              vm.announcement.isActive = !vm.announcement.isActive
              vm.$store.dispatch('unsetSpinnerLoading')
            })
            .catch(() => {
              mixins.methods.errorPopUp(vm)
              vm.$store.dispatch('unsetSpinnerLoading')
            })
        })
    },
    modyfi () {
      let vm = this
      let id = this.$route.params.id
      axios.put(`announcements/${id}`, {title: this.announcement.title, content: this.announcement.content})
        .then(() => {
          mixins.methods.simpleSuccessPopUp(vm)
          vm.$router.go(-1)
        })
        .catch(() => {
          mixins.methods.errorPopUp(vm)
        })
    },
    remove () {
      this.$store.dispatch('setSpinnerLoading')
      let vm = this
      let id = this.$route.params.id
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.delete(`announcements/${id}`)
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.$router.go(-1)
            })
            .catch(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.errorPopUp(vm)
            })
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    goBack () {
      this.$router.go(-1)
    }
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAdmin) {
        next()
      } else {
        next('/')
      }
    })
  },
  mounted () {
    if (this.$route.params.id) {
      let vm = this
      let id = this.$route.params.id
      this.$store.dispatch('getAnnouncement', id)
        .then(response => {
          vm.isEditing = true
          vm.announcement = response.data
          vm.announcement.dateCreated = vm.announcement.dateCreated.substring(0, 10) + ' ' +
            vm.announcement.dateCreated.substring(11, 16)
        })
        .catch(() => {
          vm.$router.push({name: 'home'})
        })
    }
  }
}
</script>

<style scoped>

</style>
