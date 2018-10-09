<template>
    <div>
        <div class="row">
            <div class="col-2" >
                <p class="font-weight-bold">Twoje zdjęcie profilowe:</p>
                <input
                        type="file"
                        style="display: none"
                        ref="fileInput"
                        @change="changePhoto">
                <img :src="`http://localhost:5000/api/users/${userId}/photo`">
            </div>
            <div class="col-1 d-flex align-items-end">
                <div>
                    <button class="btn btn-primary btn-block" @click="$refs.fileInput.click()">Zmień</button>
                    <button class="btn btn-error btn-block" @click="deletePhoto">Usuń</button>
                </div>
            </div>
        </div>

        <div class="row mt-2 pl-3">
            <p class="font-weight-bold">Twój opis: </p>
        </div>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'MyProfile',
  props: {
    userId: Number
  },
  data () {
    return {
      selectedFile: null,
      user: {
        phoneNumber: Number,
        email: String,
        description: String
      }
    }
  },
  methods: {
    changePhoto (event) {
      this.$store.dispatch('setSpinnerLoading')
      this.selectedFile = event.target.files[0]
      const fd = new FormData()
      fd.append('', this.selectedFile, this.selectedFile.name)
      let vm = this
      axios.patch(`users/${this.userId}/photo`, fd,
        {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        })
        .then(() => {
          mixins.methods.simpleSuccessPopUp(vm)
          vm.$store.dispatch('unsetSpinnerLoading')
        })
        .catch(() => {
          mixins.methods.errorPopUp(vm)
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    deletePhoto () {
      let vm = this
      mixins.methods.confirmationDialog(vm).then(() => {
        vm.$store.dispatch('setSpinnerLoading')
        axios.patch(`users/${vm.userId}/photo`)
          .then(() => {
            mixins.methods.simpleSuccessPopUp(vm)
            vm.$store.dispatch('unsetSpinnerLoading')
          })
          .catch(() => {
            mixins.methods.errorPopUp(vm)
            vm.$store.dispatch('unsetSpinnerLoading')
          })
      })
    }
  },
  mounted () {
    let vm = this
    axios.get(`users/${this.userId}`)
      .then(response => {
        vm.user.email = response.data.email
        vm.user.phoneNumber = response.data.phoneNumber
        vm.user.description = response.data.description
      })
  }
}
</script>

<style scoped>
    img {
        width: 13vw;
        height: 13vw;
        object-fit: contain;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
</style>
