<template>
    <div>
        <div class="row">
            <div class="col-2" >
                <p class="font-weight-bold">Twoje zdjęcie profilowe:</p>
                <input
                        type="file"
                        style="display: none"
                        ref="fileInput"
                        accept="image/x-png, image/gif, image/jpeg"
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
            <textarea
                    id="content"
                    :disabled="!isEditingDescription"
                    class="form-control"
                    rows="12"
                    v-model="user.description">
            </textarea>
        </div>
        <div class="row mt-3 pl-3 d-flex justify-content-end">
            <button v-if="!isEditingDescription"
                    class="btn btn-primary"
                    @click="onEditingDescription">Edytuj opis</button>
            <button v-if="isEditingDescription"
                    class="btn btn-warning mr-2"
                    @click="saveChanges('description')">Zapisz opis</button>
            <button
                    v-if="isEditingDescription"
                    class="btn btn-primary"
                    @click="offEditingDescription">Powrót</button>
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
      isEditingDescription: false,
      isEditingEmail: false,
      isEditingPhone: false,
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
      console.log(this.selectedFile)
      if (!this.selectedFile.type.includes('image')) {
        this.$store.dispatch('unsetSpinnerLoading')
        mixins.methods.customErrorPopUp(this, 'Wybrane rozszerzenie pliku nie jest wspierane!')
        return
      }
      if (this.selectedFile.size > 3000000) {
        this.$store.dispatch('unsetSpinnerLoading')
        mixins.methods.customErrorPopUp(this, 'Wybrany plik jest zbyt duży!')
        return
      }
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
          vm.$router.go(0)
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
    },
    onEditingDescription () {
      this.isEditingDescription = true
      this.$emit('somethingChanged', true)
    },
    offEditingDescription () {
      this.isEditingDescription = false
      this.$emit('somethingChanged', false)
    },
    saveChanges (value) {
      let vm = this
      vm.$store.dispatch('setSpinnerLoading')
      let urlpart = value
      let valueToSend = null

      switch (value) {
        case 'description':
          valueToSend = this.user.description
          break
        case 'email':
          valueToSend = this.user.email
          break
        case 'phone':
          valueToSend = this.user.phoneNumber
          break
        default:
          break
      }

      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.patch(`users/${this.userId}/${urlpart}`, '"' + valueToSend + '"', {
            headers: {
              'Content-Type': 'application/json'
            }
          })
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.$emit('somethingChanged', false)
              vm.isEditingDescription = false
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
    onEditingEmail () {
      this.isEditingEmail = true
      this.$emit('somethingChanged', true)
    },
    offEditingEmail () {
      this.isEditingEmail = false
      this.$emit('somethingChanged', false)
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
