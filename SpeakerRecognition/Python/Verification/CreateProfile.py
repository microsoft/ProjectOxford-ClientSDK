import VerificationServiceHttpClientHelper
import sys

def create_profile(subscription_key, locale):
    """Creates a profile on the server.

    Arguments:
    subscription_key -- the subscription key string
    locale -- the locale string
    """
    helper = VerificationServiceHttpClientHelper.VerificationServiceHttpClientHelper(
        subscription_key)

    creation_response = helper.create_profile(locale)

    print('Profile ID = {0}'.format(creation_response.get_profile_id()))

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print('Usage: python CreateProfile.py <subscription_key>')
        print('\t<subscription_key> is the subscription key for the service')
        sys.exit('Error: Incorrect Usage.')

    create_profile(sys.argv[1], 'en-us')
