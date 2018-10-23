<template>
    <div class="row no-gutters mt-3">
        <div class="col-md-3 col-12 leftCol">
                <p class="font-weight-bold">Twoje zdjęcie profilowe:</p>
            <div class="d-flex justify-content-center">
                <input
                        type="file"
                        style="display: none"
                        ref="fileInput"
                        accept="image/x-png, image/gif, image/jpeg"
                        @change="changePhoto">
                <img :src="`http://localhost:5000/api/users/${userId}/photo`">
            </div>
            <div class="mt-4 pr-1">
                <button class="btn btn-primary btn-block" @click="$refs.fileInput.click()">Zmień</button>
                <button class="btn btn-danger btn-block" @click="deletePhoto">Usuń</button>
            </div>
            <div class="mt-3 pr-1 d-flex align-items-end">
                <form class="w-100">
                    <div class="row no-gutters">
                        <div class="w-100">
                            <label for="email">Adres e-mail</label>
                            <div>
                                <input
                                        type="text"
                                        class="form-control"
                                        id="email"
                                        :disabled="!isEditingEmail"
                                        @blur="$v.user.email.$touch()"
                                        v-model="user.email">
                                <p v-if="!$v.user.email.email">Nieprawidłowy adres email!</p>
                                <p v-if="!$v.user.email.required">To pole nie może być puste.</p>
                                <button
                                        v-if="!isEditingEmail"
                                        class="btn btn-primary btn-block mt-1"
                                        type="button"
                                        @click="onEditingEmail">Zmień adres email
                                </button>
                                <button
                                        v-if="isEditingEmail"
                                        class="btn btn-primary mt-1"
                                        type="button"
                                        @click="offEditingEmail">Powrót
                                </button>
                                <button
                                        v-if="isEditingEmail"
                                        class="btn btn-warning mt-1 ml-3"
                                        :disabled="$v.$invalid"
                                        @click="saveChanges('email')">Zapisz zmiany
                                </button>
                            </div>
                        </div>
                    </div>
                    <div class="row no-gutters mt-1">
                        <div class="w-100">
                            <label for="phoneNumber">Numer telefonu</label>
                            <div>
                                <input
                                        type="text"
                                        id="phoneNumber"
                                        class="form-control"
                                        :disabled="!isEditingPhone"
                                        @blur="$v.user.phoneNumber.$touch()"
                                        v-model="user.phoneNumber">
                                <p v-show="!$v.user.phoneNumber.minLen">
                                    Nie mniej niż {{ $v.user.phoneNumber.$params.minLen.min }} znaków!
                                </p>
                                <p v-if="!$v.user.phoneNumber.maxLen">
                                    Nie więcej niż {{ $v.user.phoneNumber.$params.maxLen.max }} znaków!
                                </p>
                                <button
                                        v-if="!isEditingPhone"
                                        class="btn btn-primary btn-block mt-1"
                                        type="button"
                                        @click="onEditingPhone">Zmień numer telefonu
                                </button>
                                <button
                                        v-if="isEditingPhone"
                                        class="btn btn-primary mt-1"
                                        type="button"
                                        @click="offEditingPhone">Powrót
                                </button>
                                <button
                                        v-if="isEditingPhone"
                                        class="btn btn-warning mt-1 ml-3"
                                        :disabled="$v.$invalid"
                                        @click="saveChanges('phone')">Zapisz zmiany
                                </button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
        <div class="col-8 ml-3">
            <div class="row mt-1 pl-3">
                <p class="font-weight-bold">Twój opis: </p>
                <textarea
                        id="content"
                        title="Opis użytkownika"
                        :disabled="!isEditingDescription"
                        class="form-control"
                        rows="15"
                        v-model="user.description">
                </textarea>
            </div>
            <div class="row mt-3 pl-3 d-flex justify-content-end">
                <button v-if="!isEditingDescription"
                        class="btn btn-primary"
                        @click="onEditingDescription">Edytuj opis
                </button>
                <button v-if="isEditingDescription"
                        class="btn btn-warning mr-2"
                        @click="saveChanges('description')">Zapisz opis
                </button>
                <button
                        v-if="isEditingDescription"
                        class="btn btn-primary"
                        @click="offEditingDescription">Powrót
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
import { required, email, maxLength, minLength } from 'vuelidate/lib/validators'
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
  computed: {
    isUserEditing () {
      return this.isEditingPhone || this.isEditingEmail || this.isEditingDescription
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
      this.loadData()
    },
    onEditingEmail () {
      this.isEditingEmail = true
      this.$emit('somethingChanged', true)
    },
    offEditingEmail () {
      this.isEditingEmail = false
      this.$emit('somethingChanged', this.isUserEditing)
      this.loadData()
    },
    onEditingPhone () {
      this.isEditingPhone = true
      this.$emit('somethingChanged', this.isUserEditing)
    },
    offEditingPhone () {
      this.isEditingPhone = false
      this.$emit('somethingChanged', this.isUserEditing)
      this.loadData()
    },
    loadData () {
      let vm = this
      axios.get(`users/${this.userId}`)
        .then(response => {
          vm.user.email = response.data.email
          vm.user.phoneNumber = response.data.phoneNumber
          vm.user.description = response.data.description
        })
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
              switch (value) {
                case 'description':
                  vm.isEditingDescription = false
                  break
                case 'email':
                  vm.isEditingEmail = false
                  break
                case 'phone':
                  vm.isEditingPhone = false
                  break
                default:
                  break
              }
            })
            .catch((error) => {
              vm.$store.dispatch('unsetSpinnerLoading')
              if (error.response.status === 409) {
                mixins.methods.customErrorPopUp(vm, 'Podany adres email jest już zajęty!')
              } else {
                mixins.methods.errorPopUp(vm)
              }
            })
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    }
  },
  validations: {
    user: {
      email: {
        required,
        email
      },
      phoneNumber: {
        minLen: minLength(7),
        maxLen: maxLength(11)
      }
    }
  },
  mounted () {
    this.loadData()
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
    input[disabled] {
        background-color: #fff;
    }
    textarea[disabled] {
        background-color: #fff;
    }
    .leftCol {
        border-right: 1px solid lightgray;
    }
</style>
